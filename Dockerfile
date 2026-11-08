# Dockerfile for Tidebreak Server & Static Game Web Hosting
FROM node:18-alpine

WORKDIR /app

# Copy server dependencies and install
COPY WebGame/server/package*.json ./WebGame/server/
WORKDIR /app/WebGame/server
RUN npm install --production

# Copy remaining project files
WORKDIR /app
COPY . .

# Expose ports: 3000 (Socket.io Server), 8080 (Game Client Web Server)
EXPOSE 3000
EXPOSE 8080

CMD ["sh", "-c", "node WebGame/server/index.js & npx serve WebGame/dist -l 8080"]
