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
    data = b''  # Inicializa un búfer de datos vacío

    # Lee los datos de la imagen en fragmentos y busca un marcador para delimitar cada imagen
    while True:
        chunk = connection.recv(1024)
        if not chunk:
            break
        data += chunk

        # Busca un marcador para determinar el final de una imagen
        if b'END_OF_IMAGE' in data:
            img_data = data.split(b'END_OF_IMAGE')
            for img_chunk in img_data:
                if len(img_chunk) == 0:
                    continue

                # Convierte los datos en una imagen
                nparr = np.frombuffer(img_chunk, np.uint8)
                img_np = cv2.imdecode(nparr, cv2.IMREAD_COLOR)

                # Comprueba si la imagen es en color o en blanco y negro
                if img_np is not None:
                    img_counter += 1
                    if img_counter % 3 == 0:
                        filename = f'color{1}.png'

                    else:
                        img_np = cv2.cvtColor(img_np, cv2.COLOR_BGR2GRAY)
                        filename = f'bicolor{bicolor_count % 2 + 1}.png'
                        bicolor_count += 1

                    cv2.imwrite(filename, img_np)

            data = b''  # Reinicia el búfer de datos
    img1 = cv2.imread('bicolor1.png', cv2.IMREAD_GRAYSCALE)
    img2 = cv2.imread('bicolor2.png', cv2.IMREAD_GRAYSCALE)

    # Asegurarse de que las imágenes tienen el mismo tamaño
    if img1.shape != img2.shape:
        print("Las imágenes deben tener el mismo tamaño")
        exit()

    # Calcular la matriz de distancia
    distancia = np.abs(img1.astype(int) - img2.astype(int))

    # Normalizar la matriz de distancia para que los valores estén entre 0 y 1
    distancia_normalizada = cv2.normalize(distancia, None, alpha=0, beta=1, norm_type=cv2.NORM_MINMAX, dtype=cv2.CV_32F)

    # Crear una imagen en color para visualizar la profundidad
    profundidad_color = np.zeros((img1.shape[0], img1.shape[1], 3), dtype=np.uint8)

    # Mapear los valores de profundidad a colores en el espectro del rojo al violeta
    profundidad_color[:, :, 0] = 255 * distancia_normalizada  # canal azul
    profundidad_color[:, :, 1] = 255 * (1 - distancia_normalizada)  # canal verde
    profundidad_color[:, :, 2] = 255 * distancia_normalizada  # canal rojo
    filename = f'profcolor{1}.png'
    cv2.imwrite(filename, profundidad_color)



    
