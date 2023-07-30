window.bootstrap = {
    Modal: {
        show: function (id) {
            var myModal = new bootstrap.Modal(document.getElementById(id));
            myModal.show();
        },
        hide: function (id) {
            var myModal = bootstrap.Modal.getInstance(document.getElementById(id));
            if (myModal) {
                myModal.hide();
            }
        }
    }
};