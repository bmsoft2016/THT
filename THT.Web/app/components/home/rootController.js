(function (app) {
    'use strict';

    app.controller('rootController', rootController);

    rootController.$inject = ['$scope', '$location', 'membershipService', '$rootScope'];
    function rootController($scope, $location, membershipService, $rootScope) {

        $scope.userData = {};

        $scope.userData.displayUserInfo = displayUserInfo;
        $scope.logout = logout;


        function displayUserInfo() {
            $scope.userData.isUserLoggedIn = membershipService.isUserLoggedIn();

            if ($scope.userData.isUserLoggedIn) {
                $scope.username = $rootScope.repository.loggedUser.username;
            }
        }

        function logout() {
            membershipService.removeCredentials();
            $location.path('#/');
            $scope.userData.displayUserInfo();
        }

        $scope.userData.displayUserInfo();
    }

})(angular.module('THT'));