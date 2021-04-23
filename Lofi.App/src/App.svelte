<script lang="ts">
	import { Router, Link, Route } from "svelte-routing";
	import Header from "./Header.svelte";
	import Player from "./Player.svelte";
	import UploadForm from "./UploadForm.svelte";

	export let url = "";
</script>

<main>
	<Header />
	Hello world
	<Router url="{url}">
		<Route path="/" component={Player} />
		<Route path="/upload" component={UploadForm} />
	</Router>
</main>

<style>
	:global(:root) {
		--bg-primary: rgb(52, 52, 52);
		--bg-secondary: rgb(77, 77, 77);
		--fnt-primary: rgb(255, 255, 255);
		--fnt-secondary: rgb(178, 178, 178);
	}

	:global(body) {
		background-color: var(--bg-primary);
		color: var(--fnt-primary);
	}

	main {
		text-align: center;
		padding: 1em;
		margin: 0 auto;
	}

	h1 {
		color: #ff3e00;
		text-transform: uppercase;
		font-size: 4em;
		font-weight: 100;
	}
</style>