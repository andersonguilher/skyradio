// server.js
const WebSocket = require('ws');
const http = require('http'); // Necessário para criar um servidor HTTP se você quiser servir arquivos estáticos, mas para WebSocket puro, 'ws' já basta.

const wss = new WebSocket.Server({ port: 8080 });

console.log('Servidor WebSocket iniciado na porta 8080');

// Estrutura para armazenar o estado de cada cliente
const clients = new Map(); // Map<WebSocket, { id: string, position: { lat, lon, alt }, frequency: number, isTransmitting: boolean }>

wss.on('connection', ws => {
    const clientId = generateUniqueId(); // Função para gerar um ID único para o cliente
    console.log(`Cliente ${clientId} conectado.`);
    clients.set(ws, { id: clientId, position: null, frequency: null, isTransmitting: false });

    ws.on('message', message => {
        try {
            const data = JSON.parse(message);
            const clientState = clients.get(ws);

            if (!clientState) {
                console.warn(`Estado do cliente não encontrado para WebSocket. Desconectando.`);
                ws.close();
                return;
            }

            switch (data.type) {
                case 'init':
                    // O cliente envia seus dados iniciais (posição, frequência)
                    clientState.position = data.position;
                    clientState.frequency = data.frequency;
                    console.log(`Cliente ${clientId} inicializado: Freq=${data.frequency}, Pos=${JSON.stringify(data.position)}`);
                    break;
                case 'update':
                    // O cliente envia atualizações de posição ou frequência
                    if (data.position) clientState.position = data.position;
                    if (data.frequency) clientState.frequency = data.frequency;
                    // console.log(`Cliente ${clientId} atualizado: Freq=${clientState.frequency}, Pos=${JSON.stringify(clientState.position)}`);
                    break;
                case 'ptt_start':
                    clientState.isTransmitting = true;
                    console.log(`Cliente ${clientId} iniciou transmissão na frequência ${clientState.frequency}`);
                    break;
                case 'ptt_end':
                    clientState.isTransmitting = false;
                    console.log(`Cliente ${clientId} encerrou transmissão.`);
                    break;
                case 'audio':
                    if (clientState.isTransmitting && clientState.frequency) {
                        // Processar e retransmitir áudio
                        processAndRelayAudio(ws, data.audioData, clientState);
                    }
                    break;
                default:
                    console.warn(`Tipo de mensagem desconhecido de ${clientId}: ${data.type}`);
            }
        } catch (e) {
            console.error(`Erro ao processar mensagem do cliente ${clients.get(ws)?.id || 'desconhecido'}:`, e);
        }
    });

    ws.on('close', () => {
        console.log(`Cliente ${clients.get(ws)?.id || 'desconhecido'} desconectado.`);
        clients.delete(ws);
    });

    ws.on('error', error => {
        console.error(`Erro no WebSocket do cliente ${clients.get(ws)?.id || 'desconhecido'}:`, error);
    });
});

function generateUniqueId() {
    return Math.random().toString(36).substr(2, 9);
}

// Função para calcular a distância entre dois pontos geográficos (Haversine)
// Retorna a distância em metros
function calculateDistance(pos1, pos2) {
    if (!pos1 || !pos2 || pos1.lat === undefined || pos1.lon === undefined || pos2.lat === undefined || pos2.lon === undefined) {
        return Infinity; // Se a posição não estiver definida, a distância é infinita
    }

    const R = 6371e3; // Raio médio da Terra em metros
    const φ1 = pos1.lat * Math.PI / 180; // φ, λ em radianos
    const φ2 = pos2.lat * Math.PI / 180;
    const Δφ = (pos2.lat - pos1.lat) * Math.PI / 180;
    const Δλ = (pos2.lon - pos1.lon) * Math.PI / 180;

    const a = Math.sin(Δφ / 2) * Math.sin(Δφ / 2) +
        Math.cos(φ1) * Math.cos(φ2) *
        Math.sin(Δλ / 2) * Math.sin(Δλ / 2);
    const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));

    const d = R * c; // em metros
    return d;
}

// Função para processar e retransmitir áudio
function processAndRelayAudio(senderWs, audioData, senderState) {
    clients.forEach((receiverState, receiverWs) => {
        // Não retransmite para o próprio remetente e verifica se o receptor está na mesma frequência
        if (receiverWs !== senderWs && receiverState.frequency && receiverState.frequency.toFixed(3) === senderState.frequency.toFixed(3)) {
            // Estão na mesma frequência, calcular degradação
            const distance = calculateDistance(senderState.position, receiverState.position);
            const degradedAudio = applyAudioDegradation(audioData, distance);

            if (degradedAudio) {
                // Envia o áudio degradado para o receptor
                receiverWs.send(JSON.stringify({ type: 'audio', audioData: degradedAudio }));
            }
        }
    });
}

// Função para aplicar degradação de áudio
function applyAudioDegradation(audioData, distance) {
    // Parâmetros de degradação (ajuste conforme necessário)
    const maxDistance = 100000; // 100 km (distância máxima para áudio audível)
    const minVolumeFactor = 0.05;    // Volume mínimo (para simular estática fraca)
    const maxNoiseIntensity = 0.3; // Intensidade máxima do ruído (0 a 1)

    if (distance > maxDistance) {
        return null; // Fora do alcance, não transmite
    }

    // Calcular fator de volume (inversamente proporcional à distância)
    let volumeFactor = 1 - (distance / maxDistance);
    if (volumeFactor < minVolumeFactor) volumeFactor = minVolumeFactor; // Garante um volume mínimo

    // Calcular intensidade do ruído (aumenta com a distância)
    let noiseIntensity = (distance / maxDistance) * maxNoiseIntensity;
    if (noiseIntensity > maxNoiseIntensity) noiseIntensity = maxNoiseIntensity;

    // Para simplificar, o servidor envia o áudio original e os fatores de degradação.
    // O cliente será responsável por aplicar o volume e adicionar ruído.
    return {
        audioBuffer: audioData, // O áudio original (base64)
        volumeFactor: volumeFactor,
        noiseIntensity: noiseIntensity,
        distance: distance // Envia a distância para o cliente para que ele possa adicionar ruído/estática
    };
}