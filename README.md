# lofi
Local music discovery and playback with Monero payments.

# Environment Variables

|Name|Purpose|Default|
|---|---|---|
|DOCKER_GATEWAY_HOST||host.docker.internal|
|LOFI_APP_HOST||${DOCKER_GATEWAY_HOST:-host.docker.internal}|
|LOFI_APP_PORT||8010|
|LOFI_API_HOST||${DOCKER_GATEWAY_HOST:-host.docker.internal}|
|LOFI_API_PORT||8010|
|LOFI_MONERO_DATA_DIR|Configures where monerod will look for the blockchain data|`~/.bitmonero`|
|LOFI_MONERO_DAEMON_HOST||${DOCKER_GATEWAY_HOST:-host.docker.internal}|
|LOFI_MONERO_DAEMON_RPC_PORT||28081|
|LOFI_MONERO_WALLET_RPC_HOST||${DOCKER_GATEWAY_HOST:-host.docker.internal}|
|LOFI_MONERO_WALLER_RPC_PORT||28083|
|LOFI_PG_DATA_DIR|Configures where the PostgreSQL database files will be stored|`~/pg`|
|LOFI_MUSIC_DIRECTORY|Configures where uploaded music will be stored|`~/music`|

1. [walletAddress, ...transfer] <- continually synch transfers into db
2. match up transfers to tips (don't reuse paymentids for now)
3. when doing a payout record the walletAddress being sent from
4. create new test / dev wallets + throw if not set