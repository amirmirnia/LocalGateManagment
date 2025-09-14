// Theme Management Script
(function() {
    'use strict';
    
    // Theme constants
    const THEME_KEY = 'services-gate-theme';
    const LIGHT_THEME = 'light';
    const DARK_THEME = 'dark';
    
    // Initialize theme on page load
    function initializeTheme() {
        const savedTheme = localStorage.getItem(THEME_KEY);
        const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
        const theme = savedTheme || (prefersDark ? DARK_THEME : LIGHT_THEME);
        
        applyTheme(theme);
        
        // Listen for system theme changes
        window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', (e) => {
            if (!localStorage.getItem(THEME_KEY)) {
                applyTheme(e.matches ? DARK_THEME : LIGHT_THEME);
            }
        });
    }
    
    // Apply theme to document
    function applyTheme(theme) {
        const root = document.documentElement;
        
        if (theme === DARK_THEME) {
            root.setAttribute('data-theme', 'dark');
        } else {
            root.removeAttribute('data-theme');
        }
        
        // Update meta theme color for mobile browsers
        const metaThemeColor = document.querySelector('meta[name="theme-color"]');
        if (metaThemeColor) {
            metaThemeColor.content = theme === DARK_THEME ? '#111827' : '#ffffff';
        }
    }
    
    // Toggle theme function
    function toggleTheme() {
        const currentTheme = document.documentElement.hasAttribute('data-theme') ? DARK_THEME : LIGHT_THEME;
        const newTheme = currentTheme === DARK_THEME ? LIGHT_THEME : DARK_THEME;
        
        applyTheme(newTheme);
        localStorage.setItem(THEME_KEY, newTheme);
        
        // Dispatch custom event for theme change
        window.dispatchEvent(new CustomEvent('themechange', { 
            detail: { theme: newTheme } 
        }));
        
        return newTheme;
    }
    
    // Get current theme
    function getCurrentTheme() {
        return document.documentElement.hasAttribute('data-theme') ? DARK_THEME : LIGHT_THEME;
    }
    
    // Expose functions to window for Blazor interop
    window.ThemeManager = {
        initialize: initializeTheme,
        toggle: toggleTheme,
        apply: applyTheme,
        getCurrent: getCurrentTheme
    };
    
    // Initialize on DOM ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initializeTheme);
    } else {
        initializeTheme();
    }
})();

// Download file function for data export
window.downloadFileFromBase64 = function(fileName, base64String) {
    const link = document.createElement('a');
    link.download = fileName;
    link.href = 'data:application/octet-stream;base64,' + base64String;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};

// Smooth scroll function
window.smoothScrollTo = function(elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        element.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
};

// Toast notification system
window.ToastNotification = {
    show: function(message, type = 'info', duration = 3000) {
        const toast = document.createElement('div');
        toast.className = `toast toast-${type}`;
        toast.textContent = message;
        toast.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            padding: 12px 20px;
            border-radius: 8px;
            background: var(--${type}-color);
            color: white;
            font-size: 14px;
            z-index: 10000;
            animation: slideIn 0.3s ease;
        `;
        
        document.body.appendChild(toast);
        
        setTimeout(() => {
            toast.style.animation = 'slideOut 0.3s ease';
            setTimeout(() => {
                document.body.removeChild(toast);
            }, 300);
        }, duration);
    }
};

// Add required CSS animations
const style = document.createElement('style');
style.textContent = `
    @keyframes slideIn {
        from {
            transform: translateX(100%);
            opacity: 0;
        }
        to {
            transform: translateX(0);
            opacity: 1;
        }
    }
    
    @keyframes slideOut {
        from {
            transform: translateX(0);
            opacity: 1;
        }
        to {
            transform: translateX(100%);
            opacity: 0;
        }
    }
`;
document.head.appendChild(style);