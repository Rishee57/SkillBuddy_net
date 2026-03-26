FROM gcc:14 AS builder

# Install required tools and dependencies
RUN apt-get update && \
    apt-get install -y wget tar make g++ libssl-dev

# Install CMake 4.0.3 manually
RUN wget https://github.com/Kitware/CMake/releases/download/v4.0.3/cmake-4.0.3.tar.gz && \
    tar -zxvf cmake-4.0.3.tar.gz && \
    cd cmake-4.0.3 && \
    ./bootstrap && \
    make -j$(nproc) && \
    make install

# Set working directory
WORKDIR /app

# Copy project source
COPY . .

# Build the C++ project
RUN mkdir build && cd build && cmake .. && make

# Final stage
FROM debian:bookworm-slim
WORKDIR /app
COPY --from=builder /app/build/main .

CMD ["./main"]
