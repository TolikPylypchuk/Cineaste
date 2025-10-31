window.showPassword = (function () {
    function findButton(element) {
        return !element || element.tagName?.toLowerCase() === 'button'
            ? element
            : findButton(element.parentElement);
    }

    function clearChildren(element) {
        while (element.lastElementChild) {
            element.removeChild(element.lastElementChild);
        }
    }

    function setIconFromTemplate(button, templateName) {
        if (!button) {
            return;
        }

        const template = document.getElementById(templateName);
        if (template) {
            const icon = template.content.cloneNode(true);
            clearChildren(button);
            button.appendChild(icon);
        }
    }

    return (inputElement, clickedElement) => {
        const button = findButton(clickedElement);

        if (inputElement.type === 'password') {
            inputElement.type = 'text';
            setIconFromTemplate(button, 'password-icon-hide-password');
        } else {
            inputElement.type = 'password';
            setIconFromTemplate(button, 'password-icon-show-password');
        }
    };
})();
