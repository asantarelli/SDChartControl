# ## Como utilizar

(Obviamente, modificar a gusto)

Agregar cola global

ColaGrafico     QUEUE,PRE(QGR)
Titulo            STRING(20)
Valor             DECIMAL(13,2)
Indice            BYTE
                END

Agregar UltimateCOM y seleccionar SDChartControl (Si es multidll, agregar también en el exe, aunque no se use)

En el reporte...

    FREE(ColaGrafico)
    QGR:Titulo = 'Sueldo Neto'
    QGR:Valor = MontoTotal
    QGR:Indice = 1
    Add(ColaGrafico)
    QGR:Titulo = 'Costo Sindical'
    QGR:Valor = TotCostoSindical
    QGR:Indice = 3
    Add(ColaGrafico)
    QGR:Titulo = 'Seg, Social Emp.'
    QGR:Valor = TotCostoSSocial
    QGR:Indice = 2
    Add(ColaGrafico)
    QGR:Titulo = 'Obra Social'
    QGR:Valor = TotCostoOSocial
    QGR:Indice = 4
    Add(ColaGrafico)
    NombreArchivo = VentanaGeneraGraficoTorta(Cod_Cargo)
    !
    IF EXISTS(NombreArchivo) = 1
        REPORT$?GraficoTorta{PROP:Text} = NombreArchivo
    END

Importar VentanaGeneraGraficoTorta 


