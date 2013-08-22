//Types
function Contact(data) {
    var self = this;

    self.Id = data.Id;
    self.Value = ko.observable(data.Value);
    self.ContactType = ko.observable(data.ContactType);
    self.Icon = ko.observable("");

    self.removeContact = function (contact) {
        mediator.trigger("UserContacts:removeContact", contact);
    };

    ko.computed(function () {
        $.getJSON("/Users/GetContactType", { contact: self.Value() }, self.ContactType);
    }, this).extend({ throttle: 400 });

    ko.computed(function () {
        switch (self.ContactType()) {
            case 0:
                self.Icon(self.Value() == "" ? "" : "icon-cancel-circled"); break;
            case 1:
                self.Icon("icon-mail"); break;
            case 2:
                self.Icon("icon-twitter"); break;
            case 3:
                self.Icon("icon-skype"); break;
            case 4:
                self.Icon("icon-phone"); break;
            case 5:
                self.Icon("icon-globe"); break;
            default:
                self.Icon("");
        }
    }, this);
}

//ViewModel
function UserContactsVm(newContacts) {
    var self = this;

    self.Contacts = ko.observableArray();

    self.addContact = function (data) {
        
        if (typeof data === "undefined" || data == self) {
            data = { Id: 0, Value: "", ContactType: 0 };
        }
        self.Contacts.push(new Contact(data));
    };

    $.each(newContacts, function (key, data) {
        self.addContact(data);
    });

    self.removeContact = function (contact) {
        self.Contacts.remove(contact);
    };

    self.getIndexedName = function (index, property) {
        return "Contacts[" + index + "]." + property;
    };
    
    //Setup bindings
    mediator.bind("UserContacts:removeContact", self.removeContact);
};