var app = angular.module('myApp', []);



app.controller('myCtrl', function($scope, $http) {
    $scope.count = 0;
    $scope.myFunction = function() {


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
          alert(response.data);
        }, function errorCallback(response)
       {

       });
    }
});
