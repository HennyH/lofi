<script lang="ts">
	import "carbon-components-svelte/css/g100.css";
	import {
		Header,
		Loading,
		Content,
		ImageLoader,
		Grid,
		Row,
		Column,
		Button,
		Modal,
	} from "carbon-components-svelte";
	import { onMount } from "svelte";

	let trackId: null | number = null;
	let trackMimeType: string | null = null;
	let audioElement: HTMLAudioElement | null = null;
	let tipModalOpen: boolean = false;
	let tipId: null | number = null;
	const audioContext = new AudioContext();

	async function getTrackMimeType(trackId: number): Promise<string> {
		const response = await fetch(
			`/api/api/tracks/${trackId}/audio/mime-type`
		);
		return await response.text();
	}

	async function getRandomTrack() {
		[trackId, trackMimeType] = [null, null];
		const response = await fetch(`/api/api/tracks/random`);
		const id = (await response.json()).trackId;
		[trackId, trackMimeType] = [id, await getTrackMimeType(id)];
	}

	async function getTippingQrCode(trackId: number) {
		const resposne = await fetch(`/api/api/tracks/${trackId}/tips`, {
			method: "POST",
			body: JSON.stringify({ message: "hello" }),
			headers: {
				'Content-Type': 'application/json'
			}
		});
		tipId = parseInt(await resposne.text());
	}

	onMount(async () => {
		const track = audioContext.createMediaElementSource(audioElement);
		track.connect(audioContext.destination);
		await getRandomTrack();
		audioElement.addEventListener("ended", () => {
			getRandomTrack();
		});
	});
</script>

<audio
	src={trackId === null ? undefined : `/api/api/tracks/${trackId}/audio`}
	type={trackMimeType === null ? undefined : trackMimeType}
	autoplay
	bind:this={audioElement}
/>
<Header platformName="Lofi" />
Hello!
<Grid fullWidth={true}>
	<Row>
		<Column>
			{#if trackId !== null}
				<ImageLoader
					src={`/api/api/tracks/${trackId}/cover`}
					type="image/jpeg"
					fadeIn
				/>
				<Button on:click={() => (tipModalOpen = true)}
					>Tip Artist</Button
				>
			{:else}
				<Loading />
			{/if}
		</Column>
	</Row>
</Grid>

<Modal
	bind:open={tipModalOpen}
	modalHeading="Tip Artist"
	primaryButtonDisabled
	secondaryButtonText="Cancel"
	size="lg"
	on:click:button--secondary={() => (tipModalOpen = false)}
	on:open={() => getTippingQrCode(trackId)}
	on:close={() => (tipId = null)}
>
	{#if tipId !== null}
		<ImageLoader src={`/api/api/tips/${tipId}/payment-url/qr`} type="image/bmp" />
	{:else}
		<Loading />
	{/if}
</Modal>
<!--
	1. get track
	2. fetch cover image 
	3. update audio element
	4. tip button (popup -> QR/link, poll until pay conf) SocialWork_01 pictogram
	<Modal preventOnClose />
		<Loading />
			-> QR + link
		-> <TabletDeviceCheck>
	else:

-->
