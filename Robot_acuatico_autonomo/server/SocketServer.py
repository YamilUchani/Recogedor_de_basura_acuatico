import socket
import struct
import cv2
import numpy as np
import torch
import requests
import time
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

server_address = ('localhost', 5000)
sock.bind(server_address)
sock.listen()
torch.cuda.is_available()
device = torch.device('cuda' if torch.cuda.is_available() else 'cpu')
# model = torch.hub.load('ultralytics/yolov5s', 'yolov5s') 
model = torch.hub.load('ultralytics/yolov5', 'custom', 'best.pt')  
print("Servidor conectado")
while True:
    # Wait for a connection
    connection, client_address = sock.accept()
    print("Se recibio datos")
    data = b''
    while True:
        chunk = connection.recv(16)
        if not chunk:
            break
        data += chunk
    nparr = np.fromstring(data, np.uint8)
    img_np = cv2.imdecode(nparr, cv2.IMREAD_COLOR)
    image = cv2.cvtColor(img_np, cv2.COLOR_BGR2RGB)
    results = model(image)
    results.print()
    print(results.xyxy[0])  # im predictions (tensor)
    print(results.pandas().xyxy[0])  # im predictions (pandas)
    results.show()
    try:  # Añade este bloque
        connection.sendall(b"Hola mundo")
    except Exception as e:
        print(f"Error al enviar datos al cliente: {e}")
    time.sleep(1)  # Añade esta línea
    connection.sendall(b"Hola mundo")
    connection.close()
