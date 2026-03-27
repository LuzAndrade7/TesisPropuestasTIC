FROM node:20-alpine as builder

WORKDIR /app

COPY package*.json ./

RUN npm ci

COPY . .

RUN npm run build

FROM node:20-alpine

WORKDIR /app

RUN npm install -g http-server

COPY --from=builder /app/dist/tesis-tic-app ./dist

EXPOSE 4200

CMD ["http-server", "-p", "4200", "-c-1", "dist"]
