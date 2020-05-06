import { useEffect, useState } from 'react';
import { Service, ServiceStatus } from '../types/Service';

// Handles calls to a generic API
function useService<T>(url: string)  {

  const [result, setResult] = useState<Service<T>>({
    status: ServiceStatus.loading
  });

  useEffect(() => {
    fetch(url)
      .then(response => {
        if (response.ok) {
          return response.json();
        } else {
          throw new Error(response.statusText)
        }
      })
      .then(response => setResult({ status: ServiceStatus.loaded, payload: response }))
      .catch(error => setResult({ status: ServiceStatus.error, error }));
  }, [url]); //only run once

  return result;
}

export default useService;