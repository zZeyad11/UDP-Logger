function Generate(MinLen, MaxLen, CharList) {
    let WordList = [];
    for (let Size = MinLen; Size <= MaxLen; Size++) {
        if (Size <= 0) return null;
        var Possibltis = Math.pow(CharList.length, Size);
        let Orgin = "";
        for (let i = 0; i < Size; i++) { Orgin += "X" }
        let Current = Orgin;
        for (let O = 0; O < Possibltis; O++) {
            while (Current.includes("X")) { Current = Current.replace("X", CharList[Math.floor(Math.random() * CharList.length)]); }
            if (!WordList.includes(Current)) {
                WordList.push(Current);
                Current = Orgin;
            } else {
                Current = Orgin;
                O--;
            }
        }
    }
    return WordList.sort(function(a, b) { return a.length - b.length || a.localeCompare(b); });
}
console.log(Generate(1, 2, [0, 1, 2, 3, 4, 5, 6, 7, 8, 9]));