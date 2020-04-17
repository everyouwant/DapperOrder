var checks = 1;
function selectAll() {
    if (checks == 1) {
        $(".checkbox").prop("checked", true);
        document.getElementById("NF-select").innerHTML = '<i class="fa fa-trash-o"></i>取消全选</a></li>';
        checks = 2;
    }
    else {
        $(".checkbox").prop("checked", false);
        document.getElementById("NF-select").innerHTML = '<i class="fa fa-trash-o"></i>全选</a></li>';
        checks = 1;
    }
}
