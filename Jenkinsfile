def buildMode = env.JOB_NAME.substring(env.JOB_NAME.lastIndexOf(' ') + 1).toLowerCase()
def workspace = "C:\\JenkinsWorkspace\\SpamProtector_" + buildMode

def PowerShell(psCmd) {
    psCmd=psCmd.replaceAll("%", "%%")
    bat "powershell.exe -NonInteractive -ExecutionPolicy Bypass -Command \"\$ErrorActionPreference='Stop';[Console]::OutputEncoding=[System.Text.Encoding]::UTF8;$psCmd;EXIT \$global:LastExitCode\""
}

pipeline {
    agent {
        node {
            label 'master'
            customWorkspace workspace
        }
    }
    environment {
        SP_DB_USER              = credentials('SP_DB_USER')
        SP_DB_PASS              = credentials('SP_DB_PASS')
        SP_RABBITMQ_HOST        = credentials('SP_RABBITMQ_HOST')
        SP_RABBITMQ_USER        = credentials('SP_RABBITMQ_USER')
        SP_RABBITMQ_PASS        = credentials('SP_RABBITMQ_PASS')
        SP_RABBITMQ_EXCHANGE    = credentials('SP_RABBITMQ_EXCHANGE')
        SP_MAILBOX_MAIN_URL     = credentials('SP_MAILBOX_MAIN_URL')
        SP_MAILBOX_MAIN_PORT    = credentials('SP_MAILBOX_MAIN_PORT')
        SP_MAILBOX_MAIN_USER    = credentials('SP_MAILBOX_MAIN_USER')
        SP_MAILBOX_MAIN_PASS    = credentials('SP_MAILBOX_MAIN_PASS')
    }
    stages {
        stage('Develop') {
            stages {
                stage ('Clean & Checkout') {
                    steps {
                        cleanWs()
                        checkout scm
                    }
                }
                stage ('Restore Nugets') {
                    steps {
                        bat 'dotnet restore .\\Dotnet\\SpamProtector\\SpamProtector.sln'
                    }
                }
                stage ('Build project') {
                    steps {
                        bat 'dotnet build .\\Dotnet\\SpamProtector\\SpamProtector.sln --configuration Release'
                    }
                }
            }
        }

        stage('Release') {
            // when {
            //     expression { buildMode == 'release' }
            // }
            stages {
                stage ('Stop services') {
                    steps {
                        bat 'sc stop SpamProtector-Catalog'
                        bat 'sc stop SpamProtector-Delete'
                        bat 'sc stop SpamProtector-Marking'
                        bat 'sc stop SpamProtector-Scan'
                        bat 'sc stop SpamProtector-MessageEmailHandler'
                        bat 'sc stop SpamProtector-MessageServiceRunHandler'
                    }
                }
                stage ('Changing configs') {
                    steps {
                            script {
                            def appSettingsJson = readJSON file: '.\\Dotnet\\SpamProtector\\Shared\\appsettings.json'
                            appSettingsJson['ConnectionStrings']['SpamProtectorDBContext'] = 'Data Source=.;Initial Catalog=SpamProtectorDB;User Id='+SP_DB_USER+';Password='+SP_DB_PASS+';'
                            appSettingsJson['Messaging']['Host'] = SP_RABBITMQ_HOST
                            appSettingsJson['Messaging']['ExchangeName'] = SP_RABBITMQ_EXCHANGE
                            appSettingsJson['Messaging']['AccountLogin'] = SP_RABBITMQ_USER
                            appSettingsJson['Messaging']['AccountPassword'] = SP_RABBITMQ_PASS

                            writeJSON file: '.\\Server-NetCoreSuite\\LifeAssistantSuite\\appsettings.json', json: appSettingsJson, pretty: 4
                        }
                    }
                }
            }
        }
    }
}
