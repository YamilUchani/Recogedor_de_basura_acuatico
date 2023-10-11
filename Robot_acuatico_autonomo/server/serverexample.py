import socket

# Configura el servidor
host = '127.0.0.1'  # Cambia esto por la dirección IP de tu máquina
port = 12345  # Puedes usar cualquier número de puerto disponible
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
            data = client_socket.recv(1024)
            if not data:
                break
            print(f"Recibido: {data}")

            # Envía una respuesta al cliente
            response = "Mensaje recibido en el servidor."
            client_socket.send(response.encode('utf-8'))

        # Cierra la conexión con el cliente actual
        client_socket.close()
        print(f"Cliente en {client_address} se desconectó")

    except KeyboardInterrupt:
        # Maneja una interrupción del teclado (Ctrl+C) para cerrar el servidor
        print("Servidor cerrado")
        server_socket.close()
        break