$(document).ready(function () {

    const BroadCastMessageToAllClientHubMethod = "BroadCastMessageToAllClient";
    const ReceiveMessageForAllClientMethodCall = "ReceiveMessageForAllClient";

    const ReceiveMessageForCallerClient = "ReceiveMessageForCallerClient";
    const BroadCastMessageToCallerClient = "BroadCastMessageToCallerClient";

    const ReceiveMessageForOtherClient = "ReceiveMessageForOtherClient";
    const BroadCastMessageToOthersClient = "BroadCastMessageToOthersClient";

    const ReceiveMessageForIndividualClient = "ReceiveMessageForIndividualClient";
    const BroadcastMessageToIndividualClient = "BroadcastMessageToIndividualClient";
  
    const ReceiveConnectedClientCountAllClient = "ReceiveConnectedClientCountAllClient"; 



    const BroadCastTypedMessageToAllClient = "BroadCastTypedMessageToAllClient";
    const ReceiveTypedMessageForAllClient = "ReceiveTypedMessageForAllClient";

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/exampletypesafehub")
        .configureLogging(signalR.LogLevel.Information)
        .build();

    function start() {
        connection.start()
            .then(() => {
             
                console.log("Hub ile bağlantı kuruldu.");
               // $("#connectionId").html(`Connection Id:`, connection.connectionId);
                $("#connectionId").html(`Connection Id : ${connection.connectionId}`);
               
            })
            .catch(err => console.error("Bağlantı hatası:", err));
    }


    const groupA = "GroupA";
    const groupB = "GroupB";
    let currentGroupList = [];

    function refreshGroupList() {

        $("#groupList").empty();
        currentGroupList.forEach(x => {

            $("#groupList").append(`<p>${x}</p>`)
        })

    };

    $("#btn-groupA-add").click(function () {


        if (currentGroupList.includes(groupA)) return;



        connection.invoke("AddGroup", groupA).then(() => {

            currentGroupList.push(groupA);
            refreshGroupList();


        })
    })
    $("#btn-groupA-remove").click(function () {

        if (!currentGroupList.includes(groupA)) return;


        connection.invoke("RemoveGroup", groupA).then(() => {

            currentGroupList = currentGroupList.filter(x => x !== groupA)
            refreshGroupList();


        })

    })
    $("#btn-groupB-add").click(function () {


        if (currentGroupList.includes(groupB)) return;

        connection.invoke("AddGroup", groupB).then(() => {

            currentGroupList.push(groupB);
            refreshGroupList();


        })
    })
    $("#btn-groupB-remove").click(function () {

        if (!currentGroupList.includes(groupB)) return;
        connection.invoke("RemoveGroup", groupB).then(() => {

            currentGroupList = currentGroupList.filter(x => x !== groupB)
            refreshGroupList();


        })

    })

    $("#btn-groupA-send-message").click(function () {

        const message = "Group A Mesaj";
        connection.invoke("BroadcastMessageToGroupClients", groupA, message).catch(err => console.error("hata", err))
        console.log("Mesaj gönderildi.")

    })

    $("#btn-groupB-send-message").click(function () {

        const message = "Group B Mesaj";
        connection.invoke("BroadcastMessageToGroupClients", groupB, message).catch(err => console.error("hata", err))
        console.log("Mesaj gönderildi.")

    })

    $("#btn-groupA-remove").click(function () {

        if (!currentGroupList.includes(groupA)) return;


        connection.invoke("RemoveGroup", groupA).then(() => {

            currentGroupList = currentGroupList.filter(x => x !== groupA)
            refreshGroupList();


        })

    })


    try {
        start();
    } catch (error) {
        console.error("Başlatma hatası:", error);
        setTimeout(() => start(), 5000);
    }  

    var span_client_count = $("#span-connected-client-count")

    connection.on(ReceiveConnectedClientCountAllClient, (count) => {
        span_client_count.text(count)
        console.log("connected client", count);
    })


    connection.on(ReceiveMessageForAllClientMethodCall, (message) => {
        console.log("gelen message", message)
   
    })

    connection.on(ReceiveMessageForCallerClient, (message) => {
        console.log("(Caller) gelen message", message)
    })

    connection.on(ReceiveMessageForOtherClient, (message) => {
        console.log("(other) gelen message", message)
    })


    connection.on(ReceiveMessageForIndividualClient, (message) => {
        console.log("(Individual) gelen message", message)
    })

    connection.on(ReceiveTypedMessageForAllClient, (product) => {
        console.log("gelen ürün", product)

    });

    $("#btn-send-message-all-client").click(function () {
        const message = "hello world";

        connection.invoke(BroadCastMessageToAllClientHubMethod, message)// Daha önce tanımlanan hub metodunu çağırarak mesajı gönderir.
            .catch(err => console.error("Hata:", err));
        console.log("mesaj gönderildi.")
    });

    $("#btn-send-message-caller-client").click(function () {
        const message = "hello world";

        connection.invoke(BroadCastMessageToCallerClient, message)// Daha önce tanımlanan hub metodunu çağırarak mesajı gönderir.
            .catch(err => console.error("Hata:", err));
        console.log("mesaj gönderildi.");
    });
 

    $("#btn-send-message-other-client").click(function () {
        const message = "hello world";

        connection.invoke(BroadCastMessageToOthersClient, message)// Daha önce tanımlanan hub metodunu çağırarak mesajı gönderir.
            .catch(err => console.error("Hata:", err));
        console.log("mesaj gönderildi."); 
    });

    $("#btn-send-message-individual-client").click(function () {
        const message = "hello world";
        const connectionId = $("#text-connectionId").val();
        connection.invoke(BroadcastMessageToIndividualClient, connectionId, message)
            .catch(err => console.error("Hata:", err));
        console.log("mesaj gönderildi.");
    });
    $("#btn-send-type-message-all-client").click(function () {
        const product = {id:1,name:"pen 1",price:200};

        connection.invoke(BroadCastTypedMessageToAllClient, product)
            .catch(err => console.error("Hata:", err));
        console.log("ürün gönderildi.")
    });
});