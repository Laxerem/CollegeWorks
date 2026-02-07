import abc


class Repository(abc.ABC):
    @abc.abstractmethod
    def initialize(self) -> None:
        pass

    @abc.abstractmethod
    def clear(self) -> None:
        pass