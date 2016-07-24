/// <reference path="/Assets/admin/libs/angular/angular.js" />
(function () {
    angular.module('THT', ['THT.common']).config(config).run(run);
    config.$inject = ['$stateProvider', '$urlRouterProvider'];

    function config($stateProvider, $urlRouterProvider) {
        $stateProvider
            .state('base', {
                url: '',
                templateUrl: '/app/shared/views/baseView.html',
                abstract: true
            })
            //.state('login', {
            //    url: "/login",
            //    parent:'base',
            //    templateUrl: "/app/components/account/loginView.html",
            //    controller: "loginController"
            //}).state('register', {
            //    url: "/register",
            //    parent:'base',
            //    templateUrl: "/app/components/account/registerView.html",
            //    controller: "registerController"
            //})
            .state('home', {
                url: "/admin",
                parent: 'base',
                templateUrl: "/app/components/home/index.html",
                controller: "homeController"
            });
      //  $urlRouterProvider.otherwise('/');
    }


    run.$inject = ['$rootScope', '$location', '$cookies', '$http'];
    function run($rootScope, $location, $cookies, $http) {
        //handle page refesh
        $rootScope.repository = $cookies.get('repository') || {};
        if ($rootScope.repository.loggedUser) {
            $http.defaults.headers.common['Authorization'] = $rootScope.repository.loggedUser.authdata;
        }
        isAuthenticated.$inject = ['membershipService', '$rootScope', '$location'];

        function isAuthenticated(membershipService, $rootScope, $location) {
            if (!membershipService.isUserLoggedIn()) {
                $rootScope.previousState = $location.path();
                $location.path('/login');
            }
        }
    }
})();

