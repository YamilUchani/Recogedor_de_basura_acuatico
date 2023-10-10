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
def handle_client(client_socket):
    while True:
        request = client_socket.recv(2073600)
        if not request:
            break
        nparr = np.frombuffer(request, np.uint8)
        img_np = cv2.imdecode(nparr, cv2.IMREAD_COLOR)
        image = cv2.cvtColor(img_np, cv2.COLOR_BGR2RGB)
        results = model(image)
        data = results.pandas().xyxy[0].to_string(formatters=formatters)
        client_socket.send(data.encode())
    client_socket.close()

def server_program():
    host = "localhost"
    port = 12345
    server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server.bind((host, port))
    server.listen(5)
    print("Servidor iniciado.")
    
    while True:
        client, addr = server.accept()
        print(f"Conexión aceptada desde: {str(addr)}")
        client_handler = threading.Thread(target=handle_client, args=(client,))
        client_handler.start()

server_program()
