set("init.allow_root", true)

radio = mksafe(playlist(reload=60, "http://api/api/playlist/radio"))

output.icecast(
    %mp3(bitrate=128),
    mount="/radio",
    host="gateway.docker.internal",
    port=8000,
    password="hackme",
    radio
)
