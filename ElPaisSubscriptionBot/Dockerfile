# Usamos la imagen base de .NET SDK para construir la aplicación
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copiamos el archivo de proyecto y restauramos las dependencias
COPY *.csproj ./
RUN dotnet restore

# Copiamos el resto del código y lo compilamos
COPY . ./
RUN dotnet publish -c Release -o out

# Usamos una imagen base más ligera para la ejecución
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

# Instalamos Chrome y ChromeDriver
RUN apt-get update && apt-get install -y wget gnupg2 curl unzip \
    && wget -q -O - https://dl.google.com/linux/linux_signing_key.pub | apt-key add - \
    && sh -c 'echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google-chrome.list' \
    && apt-get update && apt-get install -y google-chrome-stable \
    && CHROME_VERSION=$(google-chrome --version | grep -oP '\d+\.\d+\.\d+') \
    && wget -q https://chromedriver.storage.googleapis.com/$CHROME_VERSION/chromedriver_linux64.zip \
    && unzip chromedriver_linux64.zip -d /usr/local/bin/ \
    && rm chromedriver_linux64.zip

# Limpiar paquetes no necesarios para reducir la imagen final
RUN apt-get clean && rm -rf /var/lib/apt/lists/*

# Copiamos los archivos compilados de la etapa de construcción
COPY --from=build-env /app/out .

# Establecemos las opciones para ChromeDriver (Ignorar SSL, Modo Headless, etc.)
ENV CHROME_OPTS="--headless --disable-gpu --no-sandbox --disable-dev-shm-usage --ignore-certificate-errors --disable-web-security --allow-running-insecure-content"

# Establecer una variable de entorno que indique que se está ejecutando en Docker
ENV RUNNING_IN_DOCKER=true

# Definimos el comando de entrada para ejecutar la aplicación
ENTRYPOINT ["dotnet", "ElPaisSubscriptionBot.dll"]

