FROM node:24-alpine AS build
WORKDIR /app

COPY frontend/lesson-runner-web/package*.json ./
RUN npm ci

# Adres API wchodzi do bundla w momencie builda. Domyślnie pusty = ścieżki relatywne,
# obsługiwane przez proxy w nginx.conf (ten sam origin, bez CORS). Ustaw tę wartość tylko,
# gdy backend stoi pod innym adresem niż frontend, np.:
#   docker build --build-arg VITE_API_BASE_URL=https://api.twojadomena.pl ...
ARG VITE_API_BASE_URL=""
ENV VITE_API_BASE_URL=$VITE_API_BASE_URL

COPY frontend/lesson-runner-web/ ./
RUN npm run build

FROM nginx:1.29-alpine
COPY frontend/lesson-runner-web/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
