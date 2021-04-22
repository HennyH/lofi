<script>
	import Header from "./Header.svelte"
	import SongHeader from "./SongHeader.svelte"
</script>

<main>
	<Header />
	<SongHeader songTitle="Oats We Sow" artistName="Gregory And The Hawk" />
</main>

<style>
	:global(body) {
		background-color: black;
		color: white;
	}

	main {
		text-align: center;
		padding: 1em;
		max-width: 240px;
		margin: 0 auto;
	}

	h1 {
		color: #ff3e00;
		text-transform: uppercase;
		font-size: 4em;
		font-weight: 100;
	}

	@media (min-width: 640px) {
		main {
			max-width: none;
		}
	}
</style>