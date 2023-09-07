class Cell:
    def __init__(self) -> None:
        pass

class PluripotentStemCell(Cell):
    def __init__(self) -> None:
        super().__init__()

class EndothelialCell(Cell):
    def __init__(self) -> None:
        super().__init__()

class Pericyte(Cell):
    def __init__(self) -> None:
        super().__init__()

class Astrocyte(Cell):
    def __init__(self) -> None:
        super().__init__()

class MicrogliaM1(Cell):
    def __init__(self) -> None:
        super().__init__()
    

class TumorCell:
    def __init__(self, pk):
        self.pk = pk

class MicrogliaM2(TumorCell):
    def __init__(self, pk):
        super().__init__(pk)

class PluripotentTumorStemCell(TumorCell):
    def __init__(self, pk):
        super().__init__(pk)

class NecroticCell(TumorCell):
    def __init__(self, pk):
        super().__init__(pk)

class MigratoryCell(TumorCell):
    def __init__(self, pk):
        super().__init__(pk)