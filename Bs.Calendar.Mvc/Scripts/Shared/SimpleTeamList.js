function SimpleTeamListVm(teamsArray) {
    var self = this;

    self.teamList = ko.observableArray();
    self.removedTeams = teamsArray;
    self.showTeamList = ko.observable(false);

    self.removeTeam = function (team) {
        self.teamList.remove(team);
        self.removedTeams.push(team);
    };

    self.addTeam = function (team) {
        self.teamList.push(team);
        self.removedTeams.remove(team);
    };
}