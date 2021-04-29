<script>
    async function handleSubmit({ target: form }) {
        var data = new FormData(form);
        await fetch("/api/album", {
            method: "POST",
            body: data
        });
    }
</script>

<form enctype="multipart/form-data" on:submit|preventDefault|stopPropagation="{handleSubmit}">
    <fieldset>
        <legend>Album</legend>
        <label for="albumTitle">Album Title:</label>
        <input id="albumTitle" name="albumTitle" type="text" required />
        <label for="albumDescription">Album Description:</label>
        <textarea id="albumDescription" name="albumDescription" required ></textarea>
        <label for="albumCoverPhoto">Cover Photo:</label>
        <input id="albumCoverPhoto" name="albumCoverPhoto" type="file" required />
        <label for="artistName">Artist Name:</label>
        <input id="artistName" name="artistName" type="text" required />
        <label for="artistWallet">Artist Wallet:</label>
        <input id="artistWallet" name="artistWallet" type="text" required />
        <label for="songs">Songs:</label>
        <input if="songs" name="songs" type="file" maxl multiple required />
        <input type="submit" value="Upload Album" />
    </fieldset>
</form>

<style>
    input, textarea {
        width: 100%;
    }
</style>