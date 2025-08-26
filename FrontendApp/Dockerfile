FROM node:20
WORKDIR /app

COPY package*.json ./
RUN npm install -g @angular/cli@17 && npm install

# Montaremos el código con -v, pero copiamos por si acaso
COPY . .

EXPOSE 4200
# Servir para todos los hosts (importante en Docker)
CMD ["sh", "-c", "ng serve --host 0.0.0.0 --port 4200"]