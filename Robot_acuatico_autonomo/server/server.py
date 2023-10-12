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
# Define una función de formato para cada columna
formatters = {
    'column1': '{:,.2f}'.format,
    'column2': '{:,.2f}'.format,
    # Añade más columnas según sea necesario
}    

host = "localhost"
port = 12345

while True:
    # Crea un socket del servidor
    server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server_socket.bind((host, port))
    server_socket.listen(5)

    print(f"Esperando una conexión en {host}:{port}...")

    try:
        # Espera a que un cliente se conecte
        client_socket, client_address = server_socket.accept()
        print(f"Conexión establecida con {client_address}")

        while True:
            # Recibe datos del cliente
            request = client_socket.recv(2073600)
            if not request:
                break
            nparr = np.frombuffer(request, np.uint8)
            img_np = cv2.imdecode(nparr, cv2.IMREAD_COLOR)
            try:
                image = cv2.cvtColor(img_np, cv2.COLOR_BGR2RGB)
                results = model(image)
                data = results.pandas().xyxy[0].to_string(formatters=formatters)
                client_socket.send(data.encode())
            except:
                break
        # Cierra la conexión con el cliente actual
        client_socket.close()
        print(f"Cliente en {client_address} se desconectó")

    except KeyboardInterrupt:
        # Maneja una interrupción del teclado (Ctrl+C) para cerrar el servidor
        print("Servidor cerrado")
        server_socket.close()
        break