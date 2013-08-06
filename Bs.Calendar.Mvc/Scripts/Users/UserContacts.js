//Types
function Contact(data) {
    var self = this;

    self.Id = data.Id;
    self.Value = ko.observable(data.Value);
    self.ContactType = ko.observable(data.ContactType);

    self.test = ko.computed(function () {
        
        $.getJSON("/Users/GetContactType", { contact: self.Value() }, self.ContactType);
    }, this).extend({ throttle: 400 });
}

//ViewModel
function UserContactsVm(newContacts) {
    var self = this;
    self.Contacts = ko.observableArray();

    self.addContact = function(data) {
        if (typeof data === "undefined" || data == self) {
            data = { Id: 0, Value: "", ContactType: 0 };
        }
                
        self.Contacts.push(new Contact(data));
    };

    $.each(newContacts, function(key,value) {
        self.addContact(value);
    });

    self.removeContact = function(contact) {
        self.Contacts.remove(contact);
    };

    self.indexedName = function (index, parameter) {
        return "Contacts[" + index + "]." + parameter;
    };

    //Custom Bindings
    ko.bindingHandlers.contactImage = {
        update: function(element, valueAccessor) {
            var valueUnwrapped = ko.unwrap(valueAccessor());
            var newClass = "";

            switch (valueUnwrapped) {
                case 1:
                    newClass = "icon-mail"; break;
                case 2:
                    newClass = "icon-twitter"; break;
                case 3:
                    newClass = "icon-skype"; break;
                case 4:
                    newClass = "icon-phone"; break;
                case 5:
                    newClass = "icon-globe";
            }
            $(element).attr("class", newClass);
        }
    };
};