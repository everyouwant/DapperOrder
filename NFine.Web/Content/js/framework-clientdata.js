var clients = [];
$(function () {
    clients = $.clientsInit();
})
$.clientsInit = function () {
    var dataJson = {
        dataItems: [],
        organize: [],
        role: [],
        duty: [],
        user: [],
        authorizeMenu: [],
        authorizeButton: [],
        supporcustomer: [],
        hourse:[],
        materials: [],
        materialsCode: [],
        CBNO: [],
        OCBNO: [],
        tray: [],
        location: [],
        Sno: [],
        locationtray:[],
        CBID: [],
        OCBID: [],
        Unit:[],
    };
    var init = function () {
        $.ajax({
            url: "/ClientsData/GetClientsDataJson",
            type: "get",
            dataType: "json",
            async: false,
            success: function (data) {
                dataJson.dataItems = data.dataItems;
                dataJson.organize = data.organize;
                dataJson.user = data.user;
                dataJson.role = data.role;
                dataJson.duty = data.duty;
                dataJson.authorizeMenu = eval(data.authorizeMenu);
                dataJson.authorizeButton = data.authorizeButton;
                dataJson.supporcustomer = data.supporcustomer;
                dataJson.hourse = data.hourse;
                dataJson.materials = data.materials;
                dataJson.materialsCode = data.materialsCode;
                dataJson.CBNO = data.CBNO;
                dataJson.OCBNO = data.OCBNO;
                dataJson.tray = data.tray;
                dataJson.location = data.location;
                dataJson.Sno = data.Sno;
                dataJson.OCBID = data.OCBID;
                dataJson.CBID = data.CBID;
                dataJson.locationtray = data.locationtray;
                dataJson.Unit = data.Unit;      
          }
        });
    }
    init();
    return dataJson;
}