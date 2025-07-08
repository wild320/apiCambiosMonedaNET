pipeline {
    agent any

    environment {
        DOCKER_IMAGE = 'apicambiosmonedatt'
        CONTAINER_NAME = 'dockerapicambiomonedastt'
        APP_PORT = "5235"
        DOCKER_NETWORK = 'tt_ani_bdmonedas_docker-compose_red'
        HOST_PORT = '7080'
    }

    stages {
        stage('Ejecutar pruebas unitarias') {
            steps {
                bat "dotnet test apiCambiosMoneda.Test\\apiCambiosMoneda.Test.csproj --configuration Release"
            }
        }
        
        stage('Construir imagen') {
            steps {
                bat "docker build . -t ${DOCKER_IMAGE}"
            }
        }
        
        stage('Limpiar contenedor existente') {
            steps {
                script {
                    bat "docker rm -f ${CONTAINER_NAME} || exit 0"
                }
            }
        }

        stage('Desplegar contenedor') {
            steps {
                bat "docker run --network ${DOCKER_NETWORK} --name ${CONTAINER_NAME} -p ${HOST_PORT}:${APP_PORT} -d ${DOCKER_IMAGE}"
            }
        }
    }
}