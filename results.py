from mpl_toolkits.mplot3d import Axes3D
import matplotlib.pyplot as plt
import numpy as np



def scatter_plot(points):
    # for pos in points:
    #     if(pos[0])
    print(points)
    # points = points.astype(int)
    x,y,z=zip(*points)
    # x=[]
    # y=[]
    # z=[]
    # for pos in points:
    #     x.append(pos[0])
    #     y.append(pos[1])
    #     z.append(pos[2])

    fig = plt.figure()
    ax = fig.add_subplot(111, projection='3d')

    ax.scatter(x, y, z)
    ax.set_xlabel('X Label')
    ax.set_ylabel('Y Label')
    ax.set_zlabel('Z Label')
    plt.show()

def histogram(points):
    # Datos de ejemplo
    x = [1, 2, 2, 3, 3, 3, 4, 4, 4, 4]
    y = [1, 2, 2, 3, 3, 3, 4, 4, 4, 4]
    z = [1, 2, 2, 3, 3, 3, 4, 4, 4, 4]

    points = [(1,1,1),(2,2,2),(2,2,2),(3,3,3),(3,3,3),(3,3,3)]


    fig = plt.figure()
    ax = fig.add_subplot()

    ax.hist2d(x, y, bins=30)
    plt.show()

def line_graph(time, growth, verhuslt, time1, growth1, time2, growth2):
    plt.plot(time, growth, label='r=2.95 *10^-2')
    plt.plot(time, verhulst, label='r=2.95 *10^-2 Verhulst')
    # plt.plot(time1, growth1, label='r=2.0 *10^-2')
    # plt.plot(time2, growth2, label='r=3.7 *10^-2')
    plt.xlabel('Tiempo')
    plt.ylabel('Tamaño del tumor')
    plt.title('Crecimiento del tumor con el tiempo')
    plt.legend()
    plt.show()

def line_graphVascular(time, growth, verhulst):
    plt.plot(time, growth, label='r=2.95 *10^-2')
    plt.plot(time, verhulst, label='r=2.95 *10^-2 Verhulst')
    # plt.xticks(np.arange(0, 100, 1))
    plt.xlim(0, 100)
    # plt.plot(time1, growth1, label='r=2.0 *10^-2')
    # plt.plot(time2, growth2, label='r=3.7 *10^-2')
    plt.xlabel('Tiempo')
    plt.ylabel('Tamaño del tumor')
    plt.title('Crecimiento del tumor con el tiempo')
    plt.legend()
    plt.show()

def line_graphVascular2(time, growth,time1, verhulst):
    plt.plot(time1, growth, label='r=2.95 *10^-2')
    plt.plot(time, verhulst, label='r=2.95 *10^-2 Verhulst')
    # plt.xticks(np.arange(0, 100, 1))
    plt.xlim(0, 100)
    plt.ylim(6800,21000)
    # plt.plot(time1, growth1, label='r=2.0 *10^-2')
    # plt.plot(time2, growth2, label='r=3.7 *10^-2')
    plt.xlabel('Tiempo')
    plt.ylabel('Tamaño del tumor')
    plt.title('Crecimiento del tumor con el tiempo')
    # plt.legend()
    plt.show()

def line_graphE(time1, growth, time, verhulst):
    plt.plot(time, growth, label='r=2.95 *10^-2')
    plt.plot(time1, verhulst, label='r=2.95 *10^-2 Verhulst')
    # plt.plot(time1, growth1, label='r=2.0 *10^-2')
    # plt.plot(time2, growth2, label='r=3.7 *10^-2')
    plt.xlabel('Tiempo')
    plt.ylabel('Tamaño del tumor')
    plt.title('Crecimiento del tumor con el tiempo')
    # plt.legend()
    plt.show()

def line_graph2(time, prolif, hip,necr,migr):
    plt.plot(time, prolif, label='Células proliferativas')
    plt.plot(time, hip, label='Células hipóxicas')
    plt.plot(time, necr, label='Células necróticas')
    plt.plot(time, migr, label='Células migratorias')
    plt.xlim(0, 500)
    plt.ylim(0, 10000)
    plt.xlabel('Tiempo')
    plt.ylabel('Numero de células tumorales')
    # plt.title('Tiempo')
    # plt.legend()
    plt.show()

def line_graph3(time, neuron, astrocyte,stem):
    plt.plot(time, neuron, label='Neuronas')
    plt.plot(time, astrocyte, label='Astrocitos')
    plt.plot(time, stem, label='Celulas madres')
    plt.xlim(0, 40)
    plt.xlabel('Tiempo')
    plt.ylabel('Número de células normales')
    # plt.title('Tiempo')
    plt.legend()
    plt.show()

def line_graph4(time, bloodVessels, neoBloodVessels,endoCells):
    # plt.plot(time, bloodVessels, label='blood vessels')
    plt.plot(time, neoBloodVessels, label='neo blood vessels')
    # plt.plot(time, endoCells, label='endothelial cells')
    plt.xlabel('Tiempo')
    plt.ylabel('Número de células')
    # plt.title('Time')
    plt.legend()
    plt.show()

# # Abrir el archivo en modo lectura ('r')
# archivo = open('nombre_del_archivo.txt', 'r')

# # Leer el contenido del archivo
# contenido = archivo.read()

# # Cerrar el archivo
# archivo.close()

# # ////////////
# # Usar 'with' para abrir el archivo
# with open('nombre_del_archivo.txt', 'r') as archivo:
#    # Leer el contenido del archivo
#    contenido = archivo.read()

# # Abrir ambos archivos
# with open('archivo_origen.txt', 'r') as archivo_origen, open('archivo_destino.txt', 'w') as archivo_destino:
#    # Leer el contenido del archivo origen
#    for linea in archivo_origen:
#        # Escribir la línea en el archivo destino
#        archivo_destino.write(linea)



def get_file(name):
        # Abre el fichero
    fichero = open(name, 'r')
    lineas = fichero.readlines()
    fichero.close()

    list = []

    # Itera sobre las líneas
    for linea in lineas:
        if linea.strip() != "StemCell:" and linea.strip() != "Neuron:" and linea.strip() != "Astrocyte:" and not linea.isdigit():
            valores = linea.split(';')
            for pos in valores:
                if(pos != '\n' and pos != 'EOF' and pos != '[EOF]'):
                    x,y,z = pos.split(',')
                    x = int(x)
                    y = int(y)
                    z = int(z)                 
                    list.append((x,y,z))

    return list

def get_growth_file(name):
    fichero = open(name, 'r')
    lineas = fichero.readlines()
    fichero.close()

    time = []
    time1 = []
    growth = []
    verhulst = []

    # Itera sobre las líneas
    for linea in lineas:
        if linea.strip() != "" and linea != '[EOF]':
            items = linea.strip().split(':')
            if(items[0] == 'time'):
                time.append(int(items[1]))
            if(items[0] == 'time1'):
                time1.append(int(items[1]))
            elif(items[0] == 'growth'):
                growth.append(int(items[1]))
            elif(items[0] == 'verhulst'):
                verhulst.append(int(items[1]))

    return time,time1,growth,verhulst

def get_growth_fileV2(name):
    fichero = open(name, 'r')
    lineas = fichero.readlines()
    fichero.close()

    time = []
    growth = []
    verhulst = []

    # Itera sobre las líneas
    for linea in lineas:
        if linea.strip() != "" and linea != '[EOF]':
            items = linea.strip().split(':')
            if(items[0] == 'time'):
                time1.append(int(items[1]))
            elif(items[0] == 'growth'):
                growth.append(int(items[1]))
            elif(items[0] == 'verhulst'):
                verhulst.append(int(items[1]))

    return time,growth,verhulst

def get_tumoralCells_file(name):
    fichero = open(name, 'r')
    lineas = fichero.readlines()
    fichero.close()

    time = []
    proliferatives = []
    hypoxic = []
    necrotic = []
    migratory = []

    # Itera sobre las líneas
    for linea in lineas:
        if linea.strip() != "" and linea != '[EOF]':
            items = linea.strip().split(':')
            if(items[0] == 'time'):
                time.append(int(items[1]))
            elif(items[0] == 'proliferatives'):
                proliferatives.append(int(items[1]))
            elif(items[0] == 'hypoxics'):
                hypoxic.append(int(items[1]))
            elif(items[0] == 'necrotics'):
                necrotic.append(int(items[1]))
            elif(items[0] == 'migratorys'):
                migratory.append(int(items[1]))

    return time,proliferatives,hypoxic,necrotic,migratory

def get_NormalCells_file(name):
    fichero = open(name, 'r')
    lineas = fichero.readlines()
    fichero.close()

    time = []
    neuron = []
    astrocyte = []
    stem = []

    # Itera sobre las líneas
    for linea in lineas:
        if linea.strip() != "" and linea != '[EOF]':
            items = linea.strip().split(':')
            if(items[0] == 'time'):
                time.append(int(items[1]))
            elif(items[0] == 'neuron'):
                neuron.append(int(items[1]))
            elif(items[0] == 'astrocyte'):
                astrocyte.append(int(items[1]))
            elif(items[0] == 'stemcells'):
                stem.append(int(items[1]))

    return time,neuron,astrocyte,stem

def get_BloodVessels_file(name):
    fichero = open(name, 'r')
    lineas = fichero.readlines()
    fichero.close()

    time = []
    vessels = []
    neoVessels = []
    endo = []

    # Itera sobre las líneas
    for linea in lineas:
        if linea.strip() != "" and linea != '[EOF]':
            items = linea.strip().split(':')
            if(items[0] == 'time'):
                time.append(int(items[1]))
            elif(items[0] == 'bloodVessels'):
                vessels.append(int(items[1]))
            elif(items[0] == 'NeoBloodVessels'):
                neoVessels.append(int(items[1]))
            elif(items[0] == 'Endothelial'):
                endo.append(int(items[1]))

    return time,vessels,neoVessels,endo
        


list = get_file("pob_vascular_15000/points.generation")
print(list)
scatter_plot(list)

time,time2, growth, verhulst = get_growth_file("pob_vascular_15000/avascular_growth.generation")
# time, growth, verhulst = get_growth_fileV2("pob_vascular_15000/avascular_growth.generation")
# print(time)
# print(growth)
# line_graph(time,growth,verhulst)

time1, time11, growth1, verhulst1 = get_growth_file("pob_vascular_15000/vascular_growth.generation")
# time1, growth1, verhulst1 = get_growth_fileV2("pob_vascular_15000/vascular_growth.generation")
# print(time)
# print(growth)
# line_graph(time1, growth1, verhulst1)

# time21, growth21, verhulst21 = get_growth_file("rate_growth_15000/avascular_growth.generation")
# # print(time)
# # print(growth)
# # line_graph(time21, growth21, verhulst21)

# time22, growth22, verhulst22 = get_growth_file("rate_growth_15000/vascular_growth.generation")
# # print(time)
# # print(growth)
# # line_graph(time22, growth22, verhulst22)

# time31, growth31, verhulst31 = get_growth_file("max_rate_growth_15000/avascular_growth.generation")
# print(time)
# print(growth)
# line_graph(time21, growth21, verhulst21)

# time32, growth32, verhulst32 = get_growth_fileV2("max_rate_growth_15000/vascular_growth.generation")

line_graphE(time,growth,time,verhulst)
line_graphE(time1, growth1, time1, verhulst1)
line_graphVascular2(time1,growth1, time1,verhulst1)

# line_graph(time,growth,verhulst,time21,growth21,time31, growth31)

# line_graph(time1,growth1,verhulst1,time22,growth22,time32,growth32)



time,prolif,hip,necr,migra=get_tumoralCells_file("pob_vascular_15000/tumoralCells.generation")
line_graph2(time,prolif,hip,necr,migra)

time,neuron,astrocyte,stem=get_NormalCells_file("pob_vascular_15000/normalCells.generation")
line_graph3(time,neuron,astrocyte,stem)

time,bloodVessels,neobloodVessels,endo=get_BloodVessels_file("pob_vascular_15000/bloodVessels.generation")
line_graph4(time,bloodVessels,neobloodVessels,endo)

# list = get_file("pob_vascular_30000/points.generation")
# print(list)
# scatter_plot(list)

# time, growth, verhulst = get_growth_file("pob_vascular_30000/growth.generation")
# print(time)
# print(growth)
# line_graph(time,growth,verhulst)

# time,prolif,hip,necr,migra=get_tumoralCells_file("pob_vascular_30000/tumoralCells.generation")
# line_graph2(time,prolif,hip,necr,migra)

# time,neuron,astrocyte,stem=get_NormalCells_file("pob_vascular_30000/normalCells.generation")
# line_graph3(time,neuron,astrocyte,stem)

# time,bloodVessels,neobloodVessels,endo=get_BloodVessels_file("pob_vascular_30000/bloodVessels.generation")
# line_graph3(time,bloodVessels,neobloodVessels,endo)


# # Abre el fichero
# fichero = open("points.generation", 'r')

# # Lee una línea del fichero
# linea = fichero.readline()

# for item in linea:
#     print(item)

# print(linea)

# # Divide la línea en una lista de valores
# valores = linea.split(';')

# # Cierra el fichero
# fichero.close()

# # Imprime los valores
# print(valores)


