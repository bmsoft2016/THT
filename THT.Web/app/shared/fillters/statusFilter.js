(function (app) {
    app.filter('statusFilter', function () {
        return function (input) {
            if (input == true)
                return 'Actived';
            else
                return 'Locked';
        }
    });
})(angular.module('THT.common'));