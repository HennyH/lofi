<script>
    export let songTitle;
    export let artistName;
</script>

<div class="song-header">
    <h1>{songTitle}</h1>
    <h2>{artistName}</h2>
</div>

<style>
    .song-header {
        display: flex;
        flex-direction: column;
    }

    h1 {
        font-weight: bold;
        font-size: 24pt;
        margin-top: 0;
        margin-bottom: 0;
    }

    h2 {
        font-weight: normal;
        font-size: 18pt;
        margin-top: 0.5em;
        margin-bottom: 0;
    }
</style>