export enum ServiceStatus{
    init,
    loading,
    loaded,
    error
}

interface ServiceInit {
    status: ServiceStatus.init;
}

interface ServiceLoading {
    status: ServiceStatus.loading;
}

interface ServiceLoaded<T> {
    status: ServiceStatus.loaded;
    payload: T;
}

interface ServiceError {
    status: ServiceStatus.error;
    error: Error;
}

export type Service<T> =
    | ServiceInit
    | ServiceLoading
    | ServiceLoaded<T>
    | ServiceError;