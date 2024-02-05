pipeline {
    agent any
    triggers {
        pollSCM("* * * * *")
    }
    environment {
        DEPLOY_NUMBER = "${BUILD_NUMBER}"
    }
    stages {
        stage("Build") {
            steps {
                sh "docker compose build"
            }
        }
        stage("Deliver") {
            steps {
                    withCredentials([usernamePassword(credentialsId: 'DOCKERHUB', passwordVariable: 'DH_PASSWORD', usernameVariable: 'DH_USERNAME')]) {
                    sh 'docker login -u $DH_USERNAME -p $DH_PASSWORD'
                    sh "docker compose push"
                }
            }
        }
        stage("Deploy") {
            steps {
                build job: 'Foodify-Deploy', parameters: [[$class: 'StringParameterValue', name: 'DEPLOY_NUMBER', value: "${BUILD_NUMBER}"]]
            }   
        }
    }
}
