set("init.allow_root", true)
song = single("/music/01_Inspirabeat.mp3")
output.icecast(
    %mp3(bitrate=128),
    mount="/radio",
    host="gateway.docker.internal",
    port=8000,
    password="hackme",
    song
)
