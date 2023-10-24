import socket
import threading
import cv2 as cv
import numpy as np
import matplotlib.pyplot as plt
import torch
import torchvision
from PIL import Image
import io
import requests
import time
import os
import glob
import ultralytics
ultralytics.checks()
from ultralytics import YOLO
import requests
import select  # Importa la biblioteca select
import random
def calcule_weigth(number):
    # Ruta actual del script
    script_directory = os.path.dirname(os.path.realpath(__file__))
    carpeta_momentum = os.path.join(script_directory, "momentum")

    # Obtener la lista de archivos en la carpeta "momentum"
    archivos_en_carpeta = os.listdir(carpeta_momentum)

    if(number == 1):
        weigth = 0.1 + len(archivos_en_carpeta) //20 * 0.1
    elif(number == 2):
        weigth = 0.9 + len(archivos_en_carpeta) //20 * 0.1
    return weigth
# Useful functions
def clear_console():
    os.system('cls' if os.name == 'nt' else 'clear')

intervalo_limpieza = 60  # 1 minuto en segundos
def obtain_angle_from_stereo(img1, img2):
    try:
        alto, ancho = img1.shape
        # Calcula la mitad del alto de las imágenes
        mitad_alto = alto // 2

        # Recorta las imágenes para tomar solo la mitad inferior
        img1 = img1[mitad_alto:, :]
        img2 = img2[mitad_alto:, :]
        # Calcular la disparidad
        stereo = cv.StereoSGBM_create(
            minDisparity=-128,
            numDisparities=256,
            blockSize=11,
            uniquenessRatio=5,
            speckleWindowSize=200,
            speckleRange=2,
            disp12MaxDiff=0,
            P1=8 * 1 * 11 * 11,
            P2=32 * 1 * 11 * 11
        )
        
        disparity_SGBM = stereo.compute(img1, img2)
        disparity_SGBM = cv.normalize(disparity_SGBM, disparity_SGBM, alpha=255, beta=0, norm_type=cv.NORM_MINMAX)
        disparity_SGBM = np.uint8(disparity_SGBM)

        # Mostrar la figura con los dos subplots
        plt.show()
        # Recortar la disparidad y realizar el procesamiento para obtener el resultado
        border_size = 140
        disparity_SGBM_cropped = disparity_SGBM[:, border_size:-border_size]
        thresh = 220
        _, disparity_SGBM_cropped = cv.threshold(disparity_SGBM_cropped, thresh, 255, cv.THRESH_BINARY)
        # Calcular el resultado basado en la disparidad
        promedio_columnas = np.mean(disparity_SGBM_cropped, axis=0)
        group_size = len(promedio_columnas) // 3
        averaged_promedios = []

        for i in range(0, len(promedio_columnas), group_size):
            group = promedio_columnas[i:i + group_size]
            average = sum(group) / len(group)
            averaged_promedios.append(average)
        print(averaged_promedios)
        min_value = min(averaged_promedios)
        min_indices = [i for i, value in enumerate(averaged_promedios) if value == min_value]

        if len(min_indices) == 3:
            selected_index = 1
        else:
            selected_index = random.choice(min_indices)

        resultado = -3 if selected_index == 0 else (0 if selected_index == 1 else 3)
        return str(resultado)
    except Exception as e:
        return 0
def obtain_angle_from_bboxes(bboxes, threshold=None):
    if len(bboxes) == 0:
        return "3"
    
    angles_election = [0, 0, 0, 0, 0, 0, 0]
    CATEGORIES = ["0", "1", "2", "3", "4", "5", "6"]
    
    for detection in bboxes:
        xmin, ymin, xmax, ymax = detection
        center_x = (xmin + xmax) // 2
        
        if threshold:
            if detection['confidence'] < threshold:
                continue
        
        if center_x <= 274:
            angles_election[0] += 1
        elif center_x <= 549:
            angles_election[1] += 1
        elif center_x <= 823:
            angles_election[2] += 1
        elif center_x <= 1098:
            angles_election[3] += 1
        elif center_x <= 1373:
            angles_election[4] += 1
        elif center_x <= 1647:
            angles_election[5] += 1
        else:
            angles_election[6] += 1
    
    selected_angle = angles_election.index(max(angles_election))
    return str(selected_angle)

def obtain_final_angle(angle1, angle2, w1, w2):
    weighted_angle = int(float(angle1) * w1 + float(angle2) * w2)//2
    
    angle_map = {-3: -45, -2: -30, -1: -15, 0: 0, 1: 15, 2: 30, 3: 45}
    final_angle = angle_map.get(weighted_angle, 0)
    
    return final_angle


# Model initialization
CATEGORIES = ["0", "1", "2", "3", "4", "5", "6"]
model = YOLO('best.pt')
img_counter = 0
inference_queue = []
img_pil =[]
img1 = []
img2 = []
angle_stereo = 0
angle_yolo = 0
# Connection parameters
host = "localhost"
port = 12345
contclear = 0
contant = 0
cont = 0
nextContTime = time.time() +10.0
# Main loop
# Main loop
while True:
    try:
        server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        server_socket.bind((host, port))
        server_socket.listen(5)
        print(f"Waiting for a connection on {host}:{port}")

        client_socket, client_address = server_socket.accept()
        print(f"Connection established with {client_address}")

        data = b''
        bytes_to_receive = 1024

        while True:
            if contclear >= 30:
              contclear = 0
              print("hola pew")
              clear_console()
            chunk = client_socket.recv(bytes_to_receive)
            if time.time() >= nextContTime:
                nextContTime = time.time() + 10.0
                if contant  <= cont:
                    print("El servidor está conectado")
                    contant = cont
                else:
                    print("Reconnecting with the client...")
                    server_socket.close()
                    break
            if not chunk:
                break
            data += chunk

            if b'END_OF_IMAGE' in data:
                img_data = data.split(b'END_OF_IMAGE')

                for img_chunk in img_data:
                    if len(img_chunk) == 0:
                        continue

                    try:
                        nparr = np.frombuffer(img_chunk, np.uint8)
                        
                        if img_pil is not None:
                            if img_counter == 2:
                                img_pil = cv.imdecode(nparr, cv.IMREAD_COLOR)
                                results = model.predict(source=img_pil)
                                detections = results[0].boxes.xyxy.cpu().numpy()
                                confidences = results[0].boxes.conf.cpu().numpy()
                                angle_yolo = obtain_angle_from_bboxes(detections)
                                angle_stereo = obtain_angle_from_stereo(img1, img2)
                                w1 = calcule_weigth(1)
                                w2 = calcule_weigth(2)
                                final_angle = obtain_final_angle(angle_stereo, angle_yolo, w1, w2)
                                momentum = 0.3
                                next_angle = sum(element * (momentum ** i) for i, element in enumerate(inference_queue))
                                inference_queue.insert(0, final_angle)
                                data = round(next_angle, 2)
                                print("weight 1: " + str(w1))
                                print("weight 2: " + str(w2))
                                print("angle: " + str(next_angle))
                                client_socket.send(str(data).encode())
                                img_counter = 0
                                img1 = []
                                img2 = []
                                img_pil =[]
                                contclear += 1
                                cont += 1
                            else:
                                if img_counter == 0:
                                    img1 = cv.imdecode(nparr, cv.IMREAD_GRAYSCALE)
                                elif img_counter == 1:
                                    img2 = cv.imdecode(nparr, cv.IMREAD_GRAYSCALE)

                                    
                                img_counter += 1

                        else:
                            continue

                    except Exception as e:
                        print(f"Error: {e}")
                        continue

                    except IOError:
                        continue

                data = b''
                img_chunk = []

    except ConnectionResetError:
        continue

    except KeyboardInterrupt:
        print("Server closed")
        server_socket.close()
        break

    except Exception as e:
        print(f"Error: {e}")
        continue
