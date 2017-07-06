var app = angular.module('myApp', ['ngRoute','ngCookies']);

app.config(['$routeProvider', function($routeProvider) {
            $routeProvider.

            when('/login', {
               templateUrl: 'Login.html',
               controller: 'LoginControl'
            }).

            when('/history', {
               templateUrl: 'History.html',
               controller: 'HistoryControl'
            }).

            otherwise({
               redirectTo: '/login'
            });
         }]);



app.controller('LoginControl', function($scope, $http, $location, $cookies, $window) {
    $scope.count = 0;
    $scope.Login = function() {

      ToSend = {
        "User": $scope.User,
        "PassworldHash": $scope.PassworldHash
      };
    alert(JSON.stringify(ToSend));
      $http(
      {
        method: 'POST',
        data: ToSend,
        url: 'http://localhost:52533/api/Login'
      }).then(function successCallback(response)
       {
         $window.sessionStorage.setItem('token',response.data);

        alert($window.sessionStorage.getItem('token'));

          if (response.data == '-1' || response.data == '-2') {
              alert('Wrong username or passworld!');
          }
          else {
             $location.url('/history');
          }

        }, function errorCallback(response)
       {

       });
    }
});



app.controller('HistoryControl', function($scope, $http, $cookies, $window){

  $scope.SearchHistory = function() {

    $scope.HistoryData = [];
    var HistoryInfo = ['1', $scope.Patient, $scope.Doctor, $window.sessionStorage.getItem('token')];
    $http({
      method: 'POST',
      data: HistoryInfo,
      url: 'http://localhost:52533/api/History'
    }).then(function successCallback(response)
     {

       $scope.HistoryData = response.data;
      }, function errorCallback(response)
     {

     });

  }

});
