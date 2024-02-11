
    public enum PopupState
    {
        INACTIVE, //jos nije prikazan, ali je u stacku
        ACTIVE, //prikazan sada
        HIDDEN //prikazan ranije, ali je preko njega prikazan neki drugi popup, pa je ovaj sakriven. Bice ponovo aktivan kada se onaj preko njega zatvori.
    }
