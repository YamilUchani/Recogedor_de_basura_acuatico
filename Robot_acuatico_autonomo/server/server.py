import socket
import threading
import cv2
import numpy as np
import torch
import requests
import time
torch.cuda.is_available()
device = torch.device('cuda' if torch.cuda.is_available() else 'cpu')
# model = torch.hub.load('ultralytics/yolov5s', 'yolov5s') 
model = torch.hub.load('ultralytics/yolov5', 'custom', 'best.pt')  
inference_queue = []


# Define una función de formato para cada columna
formatters = {
    'column1': '{:,.2f}'.format,
    'column2': '{:,.2f}'.format,
    # Añade más columnas según sea necesario
}    

host = "localhost"
port = 12345
angle=0
while True:
    # Crea un socket del servidor
    server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server_socket.bind((host, port))
    server_socket.listen(5)

    print(f"Esperando una conexión en {host}:{port}...")
    try:
        # Wait for a client to connect
        client_socket, client_address = server_socket.accept()
    
        print(f"Connection established with {client_address}")
        data = b''  # Initialize an empty byte variable to store received data
        bytes_to_receive = 2073600  # Number of bytes you want to receive in each iteration
        while True:
            # Receive data from the client
            try:
                request = client_socket.recv(bytes_to_receive)
            except ConnectionAbortedError:
                # Handle disconnection due to client abort
                print(f"Client at {client_address} disconnected")
                break;
            if not request:
                print(f"Client at {client_address} disconnected")
                break  # The client has disconnected, exit the inner loop
            nparr = np.frombuffer(request, np.uint8)
            img_np = cv2.imdecode(nparr, cv2.IMREAD_COLOR)
            try:
                image = cv2.cvtColor(img_np, cv2.COLOR_BGR2RGB)
                results = model(image)
                rangos = [0, 0, 0, 0, 0, 0, 0]  # Inicializa la lista de rangos con ceros
                # Add an extra column with a value of 5 to the results
                element = results.pandas().xyxy[0]
                for index, row in element.iterrows():
                    puntoMedioX = (row['xmin'] + row['xmax']) / 2
                    confidence = row['confidence']
                    if 0 <= puntoMedioX <= 274:
                        rangos[0] += confidence
                    elif 275 <= puntoMedioX <= 549:
                        rangos[1] += confidence
                    elif 550 <= puntoMedioX <= 823:
                        rangos[2] += confidence
                    elif 824 <= puntoMedioX <= 1098:
                        rangos[3] += confidence
                    elif 1099 <= puntoMedioX <= 1373:
                        rangos[4] += confidence
                    elif 1374 <= puntoMedioX <= 1647:
                        rangos[5] += confidence
                    elif 1648 <= puntoMedioX <= 1920:
                        rangos[6] += confidence
                rangoConMasPuntos = rangos.index(max(rangos))
                if rangoConMasPuntos == 0:
                    angulo=-45
                elif rangoConMasPuntos == 1:
                    angulo=-30
                elif rangoConMasPuntos == 2:
                    angulo=-15
                elif rangoConMasPuntos == 3:
                    angulo=0
                elif rangoConMasPuntos == 4:
                    angulo=15
                elif rangoConMasPuntos == 5:
                    angulo=30
                elif rangoConMasPuntos == 6:
                    angulo=45
                else:
                    print("Ningún rango tiene puntos medios.")
                # Agrega el nuevo dato al final de la lista
                inference_queue.append(angulo)
                
                # Si la lista tiene más de 10q elementos, elimina el más antiguo
                if len(inference_queue) > 10:
                    inference_queue.pop(0)
                sumangle = sum(element * (0.4 ** (i + 1)) for i, element in enumerate(inference_queue))
                data = results.pandas().xyxy[0].to_string(formatters=formatters)
                data = data +"^"+str(sumangle)
                client_socket.send(data.encode()) 
            except Exception as e:
                print(f"Error processing image: {e}")

    except ConnectionResetError:
        # Handle an unexpected client disconnection
        print(f"Client at {client_address} disconnected unexpectedly")
    
    except KeyboardInterrupt:
        # Handle a keyboard interruption (Ctrl+C) to close the server
        print("Server closed")
        server_socket.close()
        break
    except Exception as e:
        print(f"Error: {e}")