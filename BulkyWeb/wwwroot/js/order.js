var dataTable;
$(document).ready(function () {
    var statuses = ["inprocess", "completed", "pending", "approved"];
    var url = window.location.search;
    var statusFound = false;

    for (var i = 0; i < statuses.length; i++) {
        if (url.includes(statuses[i])) {
            loadDataTable(statuses[i]);
            statusFound = true;
            break;
        }
    }

    if (!statusFound) {
        loadDataTable("all");
    }
});


function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/order/getAll?status=' + status },
        "columns" : [
            { data: 'id', "width": "5%" },
            { data: 'name', "width": "25%" },
            { data: 'phoneNumber', "width": "20%" },
            { data: 'applicationUser.email', "width": "20%" },
            { data: 'orderStatus', "width": "10%" },
            { data: 'orderTotal', "width": "10%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                    <a href="/admin/order/details?orderId=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i></a>
                    </div>`
                },
                "width": "10%"
            },
        ]
    });
}