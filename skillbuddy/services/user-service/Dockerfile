# Build stage
FROM golang:1.22-alpine AS builder

WORKDIR /app
COPY . .

RUN go mod init user-service && go mod tidy
RUN go build -o main .

# Final stage
FROM alpine:latest

WORKDIR /app
COPY --from=builder /app/main .

CMD ["./main"]
