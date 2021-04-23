<script>
    export let photoUrl;
    export let photoAlt;
</script>

<div class="cover-photo">
    <img src={photoUrl} alt={photoAlt} />
    <div class="overlay">
        <button>⛶</button>
    </div>
</div>

<style>
    img {
        width: 400px;
    }

    .cover-photo {
        display: inline-block;
        margin: 0 auto;
        position: relative;
    }

    .cover-photo:hover .overlay {
        display: initial;
    }

    .overlay {
        position: absolute;
        display: none;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background-color: rgba(52, 52, 52, 0.4);
    }

    button {
        all: unset;
        position: absolute;
        top: 0;
        right: 0;
        font-size: 18pt;
    }
</style>