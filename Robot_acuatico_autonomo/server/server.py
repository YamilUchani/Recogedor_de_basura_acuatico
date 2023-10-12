import socket
import threading
import cv2
import numpy as np
import torch
import requests
import time

torch.cuda.is_available()
device = torch.device('cuda' if torch.cuda.is_available() else 'cpu')
model = torch.hub.load('ultralytics/yolov5', 'custom', 'best.pt')

formatters = {
    'column1': '{:,.2f}'.format,
    'column2': '{:,.2f}'.format,
    # Add more columns as needed
}

host = "localhost"
port = 12345

# Create a server socket outside of the main loop
server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_socket.bind((host, port))
server_socket.listen(5)

while True:
    print(f"Waiting for a connection on {host}:{port}...")

    try:
        # Wait for a client to connect
        client_socket, client_address = server_socket.accept()
        print(f"Connection established with {client_address}")

        while True:
            request = client_socket.recv(2073600)
            if not request:
                # If no data is received, the client has disconnected
                print(f"Client at {client_address} disconnected")
                client_socket.close()
                break

            nparr = np.frombuffer(request, np.uint8)
            img_np = cv2.imdecode(nparr, cv2.IMREAD_COLOR)

            try:
                image = cv2.cvtColor(img_np, cv2.COLOR_BGR2RGB)
                results = model(image)
                data = results.pandas().xyxy[0].to_string(formatters=formatters)
                client_socket.send(data.encode())
            except Exception as e:
                print(f"Error processing image: {e}")
                # Handle errors gracefully

    except KeyboardInterrupt:
        # Handle keyboard interrupt to close the server socket
        print("Server closed")
        server_socket.close()
        break
    except Exception as e:
        print(f"Error: {e}")

