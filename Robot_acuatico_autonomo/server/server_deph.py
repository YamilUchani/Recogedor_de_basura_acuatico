import socket
import cv2
import numpy as np

# Crea variables para almacenar las imágenes
img_izq = None
img_der = None
img_cen = None

server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server.bind(('127.0.0.1', 12345))
server.listen(1)

bicolor_count = 0
color_count = 0
img_counter = 0

while True:
    connection, address = server.accept()
    data = connection.recv(7000000)  # Lee los datos de la imagen (ajusta el tamaño según tus necesidades)

    # Convierte los datos en una imagen
    nparr = np.frombuffer(data, np.uint8)
    img_np = cv2.imdecode(nparr, cv2.IMREAD_COLOR)  # Ahora se espera una imagen a color

    # Comprueba si la imagen es en color o en blanco y negro
    if img_np is not None:
        img_counter += 1
        if img_counter % 3 == 0:
            # La tercera imagen es a color
            filename = f'color.png'
            cv2.imwrite(filename, img_np)
        else:
            # Las dos primeras imágenes son en blanco y negro
            img_np_bw = cv2.cvtColor(img_np, cv2.COLOR_BGR2GRAY)
            filename = f'bicolor{bicolor_count % 2 + 1}.png'
            cv2.imwrite(filename, img_np_bw)
            bicolor_count += 1
    else:
        print("Failed to decode image data")

    connection.close()
