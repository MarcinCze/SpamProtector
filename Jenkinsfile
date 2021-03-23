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
    parameters {
        booleanParam(name: 'DEPLOY_SCAN_SERVICE', defaultValue: false, description: 'Deploy ScanService?')
        booleanParam(name: 'DEPLOY_CATALOG_SERVICE', defaultValue: false, description: 'Deploy CatalogService?')
        booleanParam(name: 'DEPLOY_DELETE_SERVICE', defaultValue: false, description: 'Deploy DeleteService?')
        booleanParam(name: 'DEPLOY_MARKING_SERVICE', defaultValue: false, description: 'Deploy MarkingService?')
        booleanParam(name: 'DEPLOY_MSG_EMAIL_HANDLER_SERVICE', defaultValue: false, description: 'Deploy MessageEmailHandlerService?')
        booleanParam(name: 'DEPLOY_MSG_SERVICERUN_HANDLER_SERVICE', defaultValue: false, description: 'Deploy MessageServiceRunHandlerService?')
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
        SP_MAILBOX_SPAM_URL     = credentials('SP_MAILBOX_SPAM_URL')
        SP_MAILBOX_SPAM_PORT    = credentials('SP_MAILBOX_SPAM_PORT')
        SP_MAILBOX_SPAM_USER    = credentials('SP_MAILBOX_SPAM_USER')
        SP_MAILBOX_SPAM_PASS    = credentials('SP_MAILBOX_SPAM_PASS')
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
                        echo 'stopping services'
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
                            appSettingsJson['Mailboxes']['MainBox']['Url'] = SP_MAILBOX_MAIN_URL
                            appSettingsJson['Mailboxes']['MainBox']['Port'] = SP_MAILBOX_MAIN_PORT
                            appSettingsJson['Mailboxes']['MainBox']['UserName'] = SP_MAILBOX_MAIN_USER
                            appSettingsJson['Mailboxes']['MainBox']['Password'] = SP_MAILBOX_MAIN_PASS
                            appSettingsJson['Mailboxes']['SpamBox']['Url'] = SP_MAILBOX_SPAM_URL
                            appSettingsJson['Mailboxes']['SpamBox']['Port'] = SP_MAILBOX_SPAM_PORT
                            appSettingsJson['Mailboxes']['SpamBox']['UserName'] = SP_MAILBOX_SPAM_USER
                            appSettingsJson['Mailboxes']['SpamBox']['Password'] = SP_MAILBOX_SPAM_PASS
                            writeJSON file: '.\\Dotnet\\SpamProtector\\Shared\\appsettings.json', json: appSettingsJson, pretty: 4
                        }
                    }
                }
                stage ('Changing version') {
                    steps {
                        script {
                            [
                                ".\\Dotnet\\SpamProtector\\CatalogService\\CatalogService.csproj",
                                ".\\Dotnet\\SpamProtector\\DeleteService\\DeleteService.csproj",
                                ".\\Dotnet\\SpamProtector\\MarkingService\\MarkingService.csproj",
                                ".\\Dotnet\\SpamProtector\\ScanService\\ScanService.csproj",
                                ".\\Dotnet\\SpamProtector\\MessageEmailHandlerService\\MessageEmailHandlerService.csproj",
                                ".\\Dotnet\\SpamProtector\\MessageServiceRunHandlerService\\MessageServiceRunHandlerService.csproj"
                            ].each { csprojFile -> 
                                def fileContent = readFile csprojFile
                                def replaced = fileContent.split('\n').collect { l ->
                                    if(l.trim().startsWith('<FileVersion>')) {
                                        def changedFileVersion = l.replace('<FileVersion>', '').replace('</FileVersion>', '').trim()
                                        changedFileVersion = changedFileVersion.substring(0, changedFileVersion.lastIndexOf('.'))
                                        changedFileVersion = '<FileVersion>' + changedFileVersion + '.' + env.BUILD_ID + '</FileVersion>'
                                        return changedFileVersion
                                    } 
                                    else if (l.trim().startsWith('<Version>')) {
                                        def changedVersion = l.replace('<Version>', '').replace('</Version>', '').trim()
                                        changedVersion = changedVersion.substring(0, changedVersion.lastIndexOf('.'))
                                        changedVersion = '<Version>' + changedVersion + '.' + env.BUILD_ID + '</Version>'
                                        return changedVersion
                                    }
                                    else {
                                        return l;
                                    }
                                }.join('\n')
                                writeFile(file: csprojFile, text: replaced)
                            }
                        }
                    }
                }
                stage ('Publish services') {
                    steps {
                        bat 'dotnet publish .\\Dotnet\\SpamProtector\\SpamProtector.sln --configuration Release'
                    }
                }
                stage ('Deploy services') {
                    steps {
                        echo 'deploy services'
                    }
                }
                stage ('Start services') {
                    steps {
                        bat 'sc start SpamProtector-Catalog'
                        bat 'sc start SpamProtector-Delete'
                        bat 'sc start SpamProtector-Marking'
                        bat 'sc start SpamProtector-Scan'
                        bat 'sc start SpamProtector-MessageEmailHandler'
                        bat 'sc start SpamProtector-MessageServiceRunHandler'
                    }
                }
            }
        }
    }
}
