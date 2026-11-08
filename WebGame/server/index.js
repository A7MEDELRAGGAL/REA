// Simple Socket.io server for Tidebreak multiplayer
// Run with: node server/index.js (requires Node.js >= 14)

const express = require('express');
const http = require('http');
const { Server } = require('socket.io');

const app = express();
const server = http.createServer(app);
const io = new Server(server, {
  cors: {
    origin: '*',
    methods: ['GET', 'POST']
  }
});

// Serve static files (the built game) if needed
app.use(express.static('..'));

io.on('connection', (socket) => {
  console.log('Player connected:', socket.id);

  // Broadcast new player to others
  socket.broadcast.emit('playerJoined', { id: socket.id });

  // Relay movement updates
  socket.on('playerMove', (data) => {
    socket.broadcast.emit('playerMove', { id: socket.id, ...data });
  });

  // Handle disconnection
  socket.on('disconnect', () => {
    console.log('Player disconnected:', socket.id);
    socket.broadcast.emit('playerLeft', { id: socket.id });
  });
});

const PORT = process.env.PORT || 3000;
server.listen(PORT, () => {
  console.log(`Tidebreak multiplayer server listening on port ${PORT}`);
});
