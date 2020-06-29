function createForm() {
    var i, j, data, answers, questions, numberOfQuestions, groupId, digitalTest, numberOfGroups;

    if (this.readyState != 4 || this.status != 200) {
        return;
    }

    data = JSON.parse(this.responseText);
    numberOfQuestions = data.Questions.length;
    answers = data.Answers;
    questions = data.Questions;
    groupId = data.GroupId;
    digitalTest = data.DigitalTest;
    numberOfGroups = data.NumberOfGroups;

    var f = document.createElement("form");
    f.setAttribute('action', "/");
    f.setAttribute('method', "post");

    var d1 = document.createElement("div");
    var d2 = document.createElement("div");
    var d3 = document.createElement("div");
    d1.setAttribute('class', 'form-group');
    d2.setAttribute('class', 'form-group');
    d3.setAttribute('class', 'form-group');

    var t1 = document.createTextNode("Name:");
    var t2 = document.createTextNode("Surname:");
    var t3 = document.createTextNode("Group:");
    var t4 = document.createTextNode("SEND");

    var l1 = document.createElement("label");
    var l2 = document.createElement("label");
    var l3 = document.createElement("label");


    var i1 = document.createElement("input"); //1st textbox-name
    var i2 = document.createElement("input"); //2nd textbox-surname
    i1.setAttribute('type', "text");
    i2.setAttribute('type', "text");
    i1.setAttribute('name', "name");
    i2.setAttribute('name', "surname");
    i1.setAttribute('class', "form-control")
    i2.setAttribute('class', "form-control")
    i1.setAttribute('placeholder', "Enter first name");
    i2.setAttribute('placeholder', "Enter last name");
    i1.required = true;
    i2.required = true;

    var s1; //optional group select, if digitalTest=true - created as hidden input element
    if (digitalTest) {
        s1 = document.createElement("input");
        s1.setAttribute('name', "group");
        s1.setAttribute('type', "hidden")
        s1.value = groupId + 1;
    }
    else {
        s1 = document.createElement("select");
        s1.setAttribute('name', "group");
        s1.setAttribute('class', "form-control")
        for (var i = 0; i < numberOfGroups; i++) {
            var opt = document.createElement("option");
            opt.value = i + 1;
            opt.appendChild(document.createTextNode(i + 1));
            s1.appendChild(opt);
        }
        s1.required = true;
        s1.selectedIndex = groupId;
    }

    var b = document.createElement("button"); //submit button
    b.setAttribute('type', "submit");
    b.setAttribute('class', "btn btn-success btn-lg");

    b.appendChild(t4);

    l1.appendChild(t1);
    l2.appendChild(t2);
    l3.appendChild(t3);

    d1.appendChild(l1);
    d1.appendChild(i1);

    d2.appendChild(l2);
    d2.appendChild(i2);

    if (!digitalTest) {
        d3.appendChild(l3);
    }
    d3.appendChild(s1);

    f.appendChild(d1);
    f.appendChild(d2);
    f.appendChild(d3);

    var d, t, s, o, tmp;

    for (i = 0; i < numberOfQuestions; i++) {
        //create question
        d = document.createElement("div");
        d.setAttribute('class', 'form-group');
        t = document.createTextNode((i + 1).toString() + ". " + questions[i]);
        s = document.createElement("select");
        s.setAttribute('class', "form-control");
        s.name = "q" + i;
        s.required = true;

        //default option in select
        o = document.createElement("option");
        tmp = document.createTextNode("Choose answer");
        o.selected = true;
        o.appendChild(tmp);
        o.value = "";
        s.appendChild(o);

        //adding possible answers to question
        for (j = 0; j < answers[i].length; j++) {
            o = document.createElement("option");
            tmp = document.createTextNode(answers[i][j]);
            o.value = answers[i][j];
            o.appendChild(tmp);
            s.appendChild(o);
        }

        d.appendChild(t);
        d.appendChild(s);
        f.appendChild(d);
    }

    f.appendChild(b); //adding SEND button

    document.getElementById("questions").appendChild(f); //adding complete form to main page
}

var xmlhttp = new XMLHttpRequest();
xmlhttp.onreadystatechange = createForm;
xmlhttp.open("GET", "data.json", true);
xmlhttp.send();