window.themeHelper = {
    setTheme: (theme) => {
        document.documentElement.setAttribute('data-theme', theme);
        localStorage.setItem('theme', theme);
    },
    getTheme: () => {
        const saved = localStorage.getItem('theme');
        if (saved) return saved;
        // ← fallback to OS preference if no saved choice
        return window.matchMedia('(prefers-color-scheme: dark)').matches 
            ? 'dark' 
            : 'light';
    }
}