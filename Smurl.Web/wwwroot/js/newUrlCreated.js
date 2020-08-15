$(document).ready(function () {
  const connection = new signalR.HubConnectionBuilder()
    .withUrl("/new-url")
    .build();

  connection.on("newUrlCreated", (response) => {
    $("#txt-url").val(null);

    document.querySelector("#response-url-div").classList.remove("hide");
    document.querySelector("#loading").classList.add("hide");

    $("#response-url").val(response.url);
  });

  connection.start().catch((err) => console.error(err.toString()));

  var submitBtn = document.getElementById("mms");
  submitBtn.addEventListener("click", showLoading, false);

  function showLoading() {
    if ($("#txt-url").val() == "") return;
    document.querySelector("#loading").classList.remove("hide");
  }

  $("#form").on("submit", function (e) {
    e.preventDefault();

    document.querySelector("#response-url-div").classList.add("hide");

    $.ajax({
      type: "POST",
      url: "/",
      data: { url: $("#txt-url").val() },
      success: function (data) {},
      error: function (request, status, error) {
        document.querySelector("#loading").classList.add("hide");
      },
    });
  });
});
