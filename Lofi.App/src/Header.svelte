<script>
    import { link } from "svelte-routing"
</script>

<header>
    <nav>
        <div class="logo"></div>
        <span class="name">Lofi</span>
        <a href="/upload" use:link>Upload</a>
    </nav>
</header>

<style>
    nav {
        display: flex;
        align-items: center;
        gap: 1em;
    }

    .logo {
        width: 32px;
        height: 32px;
        background-color: rgb(178, 45, 45);
    }

    .name {
        font-weight: bolder;
        font-size: 24;
    }
</style>
